import React, { useEffect } from "react";
import { useNavigate } from "react-router";
import { useFormik } from 'formik';
import * as yup from 'yup';
import { useAppDispatch } from "redux/store";
import { tokenAPICALL } from "services/authService";
import { setJWTToken } from "redux/actions";
import { setError } from "redux/slices/auth";
import { Alert, Button, Form } from "react-bootstrap";
import { useSelector } from "react-redux";
import { RootState } from "redux/reducers";
import { Constant } from "template/Constant";
const Login: React.FC = () => {
    const navigate = useNavigate();
    const dispatch = useAppDispatch();
    const rData = useSelector((state: RootState) => state.authToken);
    const formik = useFormik({
        initialValues: {
            username: '',
            password: ''
        },
        onSubmit: async (values) => {
            const response = await tokenAPICALL(values.username, values.password);
            if (response) {
                dispatch(setJWTToken(response));
                navigate('/dashboard', { replace: true });
            } else {
                dispatch(setError("Invalid Credentials"))
            }
        },
        validationSchema: yup.object({
            username: yup.string().trim().required('Username is required'),
            password: yup.string().trim().required('Password is required'),
        }),
    });

    useEffect(() => {
        document.body.classList.toggle('bg-gradient-primary', true);
    }, [])
    return (

        <div className="container">
            <div className="row justify-content-center">
                <div className="col-xl-10 col-lg-12 col-md-9">
                    <div className="card o-hidden border-0 shadow-lg my-5">
                        <div className="card-body p-0">
                            <div className="row">
                                <div className="col-lg-6 d-none d-lg-block bg-login-image"></div>
                                <div className="col-lg-6">
                                    <div className="p-5">
                                        <div className="text-center">
                                            <h1 className="h4 text-gray-900 mb-4">Welcome Back!</h1>
                                        </div>
                                        <Form className="user" onSubmit={formik.handleSubmit}>
                                            <Form.Group>
                                                <label className="form-control-label">Username</label>
                                                <Form.Control type="text" name="username" className="form-control-user" value={formik.values.username}
                                                    onChange={formik.handleChange}
                                                    onBlur={formik.handleBlur}
                                                    isInvalid={!!formik.touched.username && !!formik.errors.username}
                                                    isValid={!!formik.touched.username && !formik.errors.username}
                                                ></Form.Control>
                                                {formik.errors.username && (
                                                    <Form.Control.Feedback type="invalid">
                                                        {formik.errors.username}
                                                    </Form.Control.Feedback>
                                                )}
                                            </Form.Group>
                                            <Form.Group>
                                                <label className="form-control-label">Password</label>
                                                <Form.Control type="password" name="password" className="form-control-user" value={formik.values.password}
                                                    onChange={formik.handleChange}
                                                    onBlur={formik.handleBlur}
                                                    isInvalid={!!formik.touched.password && !!formik.errors.password}
                                                    isValid={!!formik.touched.password && !formik.errors.password}
                                                ></Form.Control>
                                                {formik.errors.password && (
                                                    <Form.Control.Feedback type="invalid">
                                                        {formik.errors.password}
                                                    </Form.Control.Feedback>
                                                )}
                                            </Form.Group>
                                            <div className="form-group">
                                                <div className="custom-control custom-checkbox small">
                                                    <input type="checkbox" className="custom-control-input" id="customCheck" />
                                                    <label className="custom-control-label">Remember
                                                        Me</label>
                                                </div>
                                            </div>
                                            <Button type="submit" className="btn-user btn-block" variant="primary">Login</Button>


                                        </Form>
                                        <hr />
                                        {rData.errorMessage ?
                                            <Alert variant={Constant.defaultAlertVarient} className="alert-dismissible fade">{rData.errorMessage}
                                                <Button type="button" className="close" data-dismiss="alert" aria-label="Close" onClick={() => dispatch(setError(''))}>
                                                    <span aria-hidden="true">&times;</span>
                                                </Button>
                                            </Alert> : null}
                                        
                                        <div className="copyright text-center my-auto">
                                            <span>Copyright &copy; {projectName} 2022</span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

            </div>

        </div>
    );
};

export default Login;

